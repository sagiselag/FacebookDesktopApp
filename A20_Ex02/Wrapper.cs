using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public class Wrapper
    {
        private static Wrapper s_Wrapper = null;
        private static readonly object sr_Lock = new object();
        private string m_FailedMsg = "Facebook blocked this option", m_Message;
        private LoginResult m_LoginResult;
        private bool v_LoggedIn;

        public static Wrapper Instace
        {
            get
            {
                if (s_Wrapper == null)
                {
                    lock (sr_Lock)
                    {
                        if (s_Wrapper == null)
                        {
                            s_Wrapper = new Wrapper();
                        }
                    }
                }

                return s_Wrapper;
            }
        }

        public User LoggedInUser { get; set; }

        public string ApplicationID { get; private set; }

        public FacebookObjectCollection<User> Friends { get; private set; }

        private Wrapper()
        {
        }

        public bool LoginAndInit()
        {
            /// Owner: design.patterns

            /// Use the FacebookService.Login method to display the login form to any user who wish to use this application.
            /// You can then save the result.AccessToken for future auto-connect to this user:
            v_LoggedIn = true;

            ApplicationID = "2887226511328546";

            try
            {
                m_LoginResult = FacebookService.Login(
                    ApplicationID,
                    "public_profile",
                    "email",
                    "publish_to_groups",
                    "user_birthday",
                    "user_age_range",
                    "user_gender",
                    "user_link",
                    "user_tagged_places",
                    "user_videos",
                    "publish_to_groups",
                    "groups_access_member_info",
                    "user_friends",
                    "user_events",
                    "user_likes",
                    "user_location",
                    "user_photos",
                    "user_posts",
                    "user_hometown",
                    "publish_actions",
                    "user_about_me",
                    "user_relationships",
                    "user_relationship_details",
                    "publish_actions",
                    "rsvp_event",
                    "user_status");

                if (!string.IsNullOrEmpty(m_LoginResult.AccessToken))
                {
                    LoggedInUser = m_LoginResult.LoggedInUser;
                }
                else
                {
                    v_LoggedIn = false;
                }
            }
            catch
            {
                v_LoggedIn = false;
                FacebookService.Logout(() => { });
            }

            return v_LoggedIn;
        }

        public string GetUrl()
        {
            try
            {
                m_Message = LoggedInUser.PictureNormalURL;
            }
            catch
            {
                m_Message = m_FailedMsg;
            }

            return m_Message;
        }

        public string GetUserName()
        {
            try
            {
                m_Message = LoggedInUser.Name;
            }
            catch
            {
                m_Message = m_FailedMsg;
            }

            return m_Message;
        }

        public IEnumerable<Post> FetchPosts(int i_IndexOfFirstPost = 0)
        {
            FacebookObjectCollection<Post> posts;
            int indexOfCurrentPost = 0;
            try
            {
                posts = LoggedInUser.Posts;
            }
            catch
            {
                throw new Exception("Can't get posts from Facebook");
            }

            foreach (Post post in posts)
            {
                if (indexOfCurrentPost >= i_IndexOfFirstPost)
                {
                    yield return post;
                }

                indexOfCurrentPost++;
            }
        }

        public string SetStatus(string i_Status)
        {
            try
            {
                Status postedStatus = LoggedInUser.PostStatus(i_Status);
                m_Message = "Status Posted! ID: " + postedStatus.Id;
            }
            catch
            {
                m_Message = m_FailedMsg;
            }

            return m_Message;
        }

        public IEnumerable<string> FetchEvents(int i_IndexOfFirstEvent = 0)
        {
            FacebookObjectCollection<Event> userEvents;
            int indexOfCurrentEvent = 0;

            try
            {
                userEvents = LoggedInUser.Events;
            }
            catch
            {
                throw new Exception(m_FailedMsg);
            }

            foreach (Event fbEvent in userEvents)
            {
                if (indexOfCurrentEvent >= i_IndexOfFirstEvent)
                {
                    yield return fbEvent.Name;
                }

                indexOfCurrentEvent++;
            }
        }

        public IEnumerable<string> FetchPages(int i_IndexOfFirst = 0)
        {
            FacebookObjectCollection<Page> pages;
            int indexOfCurrent = 0;

            try
            {
                pages = LoggedInUser.LikedPages;
            }
            catch
            {
                throw new Exception(m_FailedMsg);
            }

            foreach (Page page in pages)
            {
                if (indexOfCurrent >= i_IndexOfFirst)
                {
                    yield return page.Name;
                }

                indexOfCurrent++;
            }
        }

        public IEnumerable<string> FetchGroups(int i_IndexOfFirst = 0)
        {
            FacebookObjectCollection<Group> groups;
            int indexOfCurrent = 0;

            try
            {
                groups = LoggedInUser.Groups;
            }
            catch
            {
                throw new Exception(m_FailedMsg);
            }

            foreach (Group group in groups)
            {
                if (indexOfCurrent >= i_IndexOfFirst)
                {
                    yield return group.Name;
                }

                indexOfCurrent++;
            }
        }

        public IEnumerable<string> FetchFriends(int i_IndexOfFirst = 0)
        {
            FacebookObjectCollection<User> friends;
            int indexOfCurrent = 0;

            try
            {
                friends = LoggedInUser.Friends;
            }
            catch
            {
                throw new Exception(m_FailedMsg);
            }

            foreach (User friend in Friends)
            {
                if (indexOfCurrent >= i_IndexOfFirst)
                {
                    yield return friend.Name;
                }

                indexOfCurrent++;
            }
        }

        public bool IsInARelationship(User user)
        {
            bool userIsInARelationship = false;

            try
            {
                if (user.RelationshipStatus.Value.CompareTo(3) <= 0 || user.RelationshipStatus.Value.CompareTo(1) > 0)
                {
                    userIsInARelationship = true;
                }
            }
            catch
            {
                throw new InvalidOperationException("Can't get user relationship status information");
            }

            return userIsInARelationship;
        }
    }
}
