﻿using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
        private int _id;
        private string _title;
        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;

        public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
        }



        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        public abstract Task LoadAsync(int id);

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public ICommand CloseDetailViewCommand { get; }

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }



        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }


        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(new
                AfterDetailDeletedEventArgs
            {
                Id = modelId,
                ViewModelName = this.GetType().Name
            });
        }

        protected virtual void RaiseCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
              .Publish(new AfterCollectionSavedEventArgs
              {
                  ViewModelName = this.GetType().Name
              });
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new
                AfterDetailSavedEventArgs
            {
                Id = modelId,
                DisplayMember = displayMember,
                ViewModelName = this.GetType().Name
            });
        }

        protected virtual void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result = MessageDialogService.ShowOKCancelDialog(
                  "You've made changes. Close this item?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<AfterDetailClosedEvent>()
        .Publish(new AfterDetailClosedEventArgs
        {
            Id = this.Id,
            ViewModelName = this.GetType().Name
        });
        }

        protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc,
        Action afterSaveAction)
        {
            try
            {
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    MessageDialogService.ShowInfoDialog("The entity has been deleted by another user");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }

                var result = MessageDialogService.ShowOKCancelDialog("The entity has been changed in "
                 + "the meantime by someone else. Click OK to save your changes anyway, click Cancel "
                 + "to reload the entity from the database.", "Question");

                if (result == MessageDialogResult.OK)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await saveFunc();
                }
                else
                {
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            };

            afterSaveAction();
        }
    }
}
