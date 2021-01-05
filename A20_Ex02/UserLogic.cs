using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public class UserLogic
    {
        public User User { get; private set; }

        public int Age { get; private set; }

        public string Gander { get; private set; }

        private readonly DateTime r_Today = new DateTime();

        public UserLogic(User i_User)
        {
            User = i_User;
            Age = getAge(User);
            Gander = getGander(User);
        }

        private int getAge(User user)
        {
            int age = -1;
            DateTime birthday;

            if (user.Birthday != null)
            {
                birthday = DateTime.Parse(user.Birthday);
                age = r_Today.Year - birthday.Year;
            }

            return age;
        }

        private string getGander(User user)
        {
            string gander = "Can't get user gender because of facebook policies";

            if (user.Gender != null)
            {
                gander = Enum.GetName(user.Gender.GetType(), user.Gender);
            }

            return gander;
        }
    }
}
