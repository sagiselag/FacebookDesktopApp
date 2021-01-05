using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public class AutoGreetingsLogic
    {
        private readonly string r_DecadeGreeting = ", it's gonna be a great decade";
        private readonly string r_BirthdayGreeting = " :)";

        public FacebookObjectCollection<Photo> Photos { get; private set; }

        public FacebookObjectCollection<Album> Albums { get; private set; }

        private readonly DateTime r_Today = new DateTime();

        private Dictionary<string, string> m_GreetingsForMan = new Dictionary<string, string>()
         {
            { "Mitzvah", " for your Bar Mitzvah :)" },
            { "ID", ", can't see your ID picture ;-)" },
            { "Driver License", ", it's now time to take Daddy car" },
            { "IDF", ", It's time to drink beer and get on a military uniform" },
            { "IDFRecruiting", ", take off the military uniform and throw on a nice T-shirt" }
        };

        private Dictionary<string, string> m_GreetingsFoWoman = new Dictionary<string, string>()
         {
            { "Mitzvah", " for your Bat Mitzvah :)" },
            { "ID", ", can't see your ID picture ;-)" },
            { "Driver License", ", it's now time to take Daddy car" },
            { "IDF", ", It's time to drink beer and get on a military uniform" },
            { "IDFRecruiting", ", take off the military uniform and throw on a sexy dress" }
        };

        private enum eGreetingEventFromAgeForMan
        {
            Mitzvah = 13,
            ID = 16,
            DriverLicense = 17,
            IDF = 18,
            IDFRecruiting = 21
        }

        private enum eGreetingEventFromAgeForWoman
        {
            Mitzvah = 12,
            ID = 16,
            DriverLicense = 17,
            IDF = 18,
            IDFRecruiting = 20
        }

        public bool FetchLookForFriendsBirthdaysAndSendGreetings(Wrapper i_Wrapper, ref List<string> i_FriendsWhoHasBirthdays, ref List<string> i_GreetingPostedOnFriendsTimeLines)
        {
            DateTime birthday;
            bool friendHasBirthday;
            bool fetcherWorksProperly = true;
            int age;

            try
            {
                i_Wrapper.FetchFriends();
                foreach (User friend in i_Wrapper.Friends)
                {
                    birthday = DateTime.Parse(friend.Birthday);
                    friendHasBirthday = (r_Today.Day == birthday.Day) && (r_Today.Month == birthday.Month);
                    if (friendHasBirthday)
                    {
                        i_FriendsWhoHasBirthdays.Add(friend.Name);
                        age = new UserLogic(friend).Age;
                        greetingFriend(friend, age, ref i_GreetingPostedOnFriendsTimeLines);
                    }
                }
            }
            catch
            {
                fetcherWorksProperly = false;
            }

            return fetcherWorksProperly;
        }

        private void greetingFriend(User i_Friend, int i_FriendAge, ref List<string> i_GreetingPostedOnFriendsTimeLines)
        {
            string startGreeting = "Happy Birthday ", specialGreeting;
            string name, gander;
            string statusToPost;
            eGreetingEventFromAgeForMan maleSpecialEvents = new eGreetingEventFromAgeForMan();
            eGreetingEventFromAgeForWoman femaleSpecialEvents = new eGreetingEventFromAgeForWoman();
            UserLogic friend = new UserLogic(i_Friend);

            gander = friend.Gander;
            name = friend.User.Name;
            if (gander == "male" && Enum.IsDefined(maleSpecialEvents.GetType(), i_FriendAge))
            {
                m_GreetingsForMan.TryGetValue(Enum.GetName(maleSpecialEvents.GetType(), i_FriendAge), out specialGreeting);
            }
            else if (Enum.IsDefined(femaleSpecialEvents.GetType(), i_FriendAge))
            {
                m_GreetingsFoWoman.TryGetValue(Enum.GetName(femaleSpecialEvents.GetType(), i_FriendAge), out specialGreeting);
            }
            else if (i_FriendAge % 10 == 0)
            {
                specialGreeting = r_DecadeGreeting;
            }
            else
            {
                specialGreeting = r_BirthdayGreeting;
            }

            statusToPost = startGreeting + i_Friend.Name + " " + specialGreeting;
            i_Friend.PostStatus(statusToPost);
            i_GreetingPostedOnFriendsTimeLines.Add(statusToPost);
        }
    }
}
