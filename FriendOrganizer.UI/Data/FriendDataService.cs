using FriendOrganizer.Model;
using System.Collections.Generic;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            yield return new Friend { FirstName = "Jesper", LastName = "Ljungberg" };
            yield return new Friend { FirstName = "Mikael", LastName = "Hentschel" };
            yield return new Friend { FirstName = "Folke", LastName = "Ljungberg" };
            yield return new Friend { FirstName = "Jonas", LastName = "Adamsson" };
        }
    }
}
