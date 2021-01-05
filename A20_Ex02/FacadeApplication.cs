using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public class FacadeApplication
    {
        private List<TargetedPhotoInformation> m_FilteredResult;
        private MostCommentablePhotosLogic m_MostCommentablePhotosLogic = new MostCommentablePhotosLogic();
        private AutoGreetingsLogic m_AutoGreetingsLogic = new AutoGreetingsLogic();
        private readonly Wrapper r_LogicWrapper;

        public FacadeApplication()
        {
            r_LogicWrapper = Wrapper.Instace;
            FacebookWrapper.FacebookService.s_CollectionLimit = 6;
            FacebookWrapper.FacebookService.s_FbApiVersion = 2.8f;
            UserPosts = new List<UpgradedPost>();
            Posts = new List<string>();
            Events = new List<string>();
            Pages = new List<string>();
            Groups = new List<string>();
            Friends = new List<string>();
            FilteredResult = new List<TargetedPhotoInformation>();
            StatusToPost = string.Empty;
            FilteredResultsQuantity = 1;
            FilteredResultsGanderIsMale = true;
            FilteredResultsNotInARelationship = false;
            ToBeSortedByIncreasingSort = true;
            FilteredResultsFromAge = "1";
            FilteredResultsToAge = "99";

            Thread thread = new Thread(GetProfilePictureAndUserName);
            thread.Start();
        }

        public FormApplication Form { get; set; }

        public string UserName { get; private set; }

        public string ProfilePictureUrl { get; private set; }

        public string StatusToPost { get; private set; }

        public int FilteredResultsQuantity { get; set; }

        public bool FilteredResultsNotInARelationship { get; set; }

        public bool ToBeSortedByIncreasingSort { get; set; }

        public bool FilteredResultsGanderIsMale { get; set; }

        public string FilteredResultsFromAge { get; set; }

        public string FilteredResultsToAge { get; set; }

        public List<UpgradedPost> UserPosts { get; private set; }

        public List<string> Posts { get; private set; }

        public List<string> Events { get; private set; }

        public List<string> Pages { get; private set; }

        public List<string> Groups { get; private set; }

        public List<string> Friends { get; private set; }

        public List<string> FriendsWhichHaveBirthday { get; private set; }

        public List<string> FriendsGreetings { get; private set; }

        public event Action PostsDelegates;

        public event Action EventsDelegates;

        public event Action PagesDelegates;

        public event Action GroupsDelegates;

        public event Action FriendsDelegates;

        public event Action SearchPhotosDelegates;

        private void notifyPostObservers()
        {
            if (PostsDelegates != null)
            {
                PostsDelegates.Invoke();
            }
        }

        private void notifyEventsObservers()
        {
            if (EventsDelegates != null)
            {
                EventsDelegates.Invoke();
            }
        }

        private void notifyPagesObservers()
        {
            if (PagesDelegates != null)
            {
                PagesDelegates.Invoke();
            }
        }

        private void notifyGroupsObservers()
        {
            if (GroupsDelegates != null)
            {
                GroupsDelegates.Invoke();
            }
        }

        private void notifyFriendsObservers()
        {
            if (FriendsDelegates != null)
            {
                FriendsDelegates.Invoke();
            }
        }

        private void notifySearchPhotosObservers()
        {
            if (SearchPhotosDelegates != null)
            {
                SearchPhotosDelegates.Invoke();
            }
        }

        public List<TargetedPhotoInformation> FilteredResult
        {
            get
            {
                return m_FilteredResult;
            }

            private set { }
        }

        public void GetProfilePictureAndUserName()
        {
            UserName = r_LogicWrapper.GetUserName();
            ProfilePictureUrl = r_LogicWrapper.GetUrl();
        }

        public string SetStatus(string i_StatusToPost) // on button set status click
        {
            string result = "Can't post a empty Status!!";

            if (i_StatusToPost.Length > 0)
            {
                result = r_LogicWrapper.SetStatus(i_StatusToPost);
                StatusToPost = string.Empty;
                GetPosts();
            }

            return result;
        }

        public List<string> GetPostsStrings(int i_IndexOfFirstPost = 0)
        {
            List<string> posts = new List<string>();
            int indexOfCurrentPost = 0;

            try
            {
                fetchPosts();
                foreach (UpgradedPost post in UserPosts)
                {
                    if (indexOfCurrentPost >= i_IndexOfFirstPost)
                    {
                        if (post.Message != null)
                        {
                            posts.Add(post.Message);
                        }
                        else if (post.Caption != null)
                        {
                            posts.Add(post.Caption);
                        }
                        else
                        {
                            posts.Add(string.Format("[{0}]", post.Type));
                        }
                    }

                    indexOfCurrentPost++;
                }
            }
            catch (NullReferenceException)
            {
                posts.Add("No Posts to retrieve :(");
            }
            catch (Exception e)
            {
                posts.Add(e.Message);
            }

            return posts;
        }

        public void GetPosts()
        {
            List<string> posts = new List<string>();

            posts.AddRange(GetPostsStrings());
            Posts = posts;
            notifyPostObservers();
        }

        private void fetchPosts()
        {
            List<Post> posts = new List<Post>();
            List<UpgradedPost> upgradedPost = new List<UpgradedPost>(UserPosts);

            try
            {
                posts.AddRange(r_LogicWrapper.FetchPosts(UserPosts.Count));
            }
            catch
            {
                throw;
            }

            foreach (Post post in posts)
            {
                upgradedPost.Add(new UpgradedPost(post));
            }

            UserPosts = upgradedPost;
        }

        public void OlderPosts() // on button Older Posts click
        {
            doubleCollectionLimit();
            GetPosts();
        }

        public void GetEvents()
        {
            List<string> events = new List<string>(Events);

            try
            {
                events.AddRange(r_LogicWrapper.FetchEvents(Events.Count));
            }
            catch (NullReferenceException)
            {
                events.Add("No Events to retrieve :(");
            }
            catch (Exception e)
            {
                events.Add(e.Message);
            }

            Events = events;
            notifyEventsObservers();
        }

        public void GetPages()
        {
            List<string> pages = new List<string>(Pages);

            try
            {
                pages.AddRange(r_LogicWrapper.FetchPages(Pages.Count));
            }
            catch (NullReferenceException)
            {
                pages.Add("No Pages to retrieve :(");
            }
            catch (Exception e)
            {
                pages.Add(e.Message);
            }

            Pages = pages;
            notifyPagesObservers();
        }

        public void GetGroups()
        {
            List<string> groups = new List<string>(Groups);

            try
            {
                groups.AddRange(r_LogicWrapper.FetchGroups(Groups.Count));
            }
            catch (NullReferenceException)
            {
                groups.Add("No Groups to retrieve :(");
            }
            catch (Exception e)
            {
                groups.Add(e.Message);
            }

            Groups = groups;
            notifyGroupsObservers();
        }

        public void GetFriends()
        {
            List<string> friends = new List<string>(Friends);

            try
            {
                friends.AddRange(r_LogicWrapper.FetchFriends(Friends.Count));
            }
            catch (NullReferenceException)
            {
                friends.Add("No Friends to retrieve :(");
            }
            catch (Exception e)
            {
                friends.Add(e.Message);
            }

            Friends = friends;
            notifyFriendsObservers();
        }

        public string BirthdayGreetings()  // on button set status click
        {
            bool featureWorksProperly;
            List<string> friendsWhoHasBirthdays = new List<string>();
            List<string> greetingPostedOnFriendsTimeLines = new List<string>();
            string result = "Feature can't work properly because of Facebook limitations";

            featureWorksProperly = m_AutoGreetingsLogic.FetchLookForFriendsBirthdaysAndSendGreetings(r_LogicWrapper, ref friendsWhoHasBirthdays, ref greetingPostedOnFriendsTimeLines);

            if (featureWorksProperly)
            {
                FriendsWhichHaveBirthday = friendsWhoHasBirthdays;
                FriendsGreetings = greetingPostedOnFriendsTimeLines;
                result = "Greetings has sent to friends";
            }

            return result;
        }

        public string SearchPhotos() // on button set Photos click
        {
            string result = "Succeed";

            try
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        getFilteredResults();
                        notifySearchPhotosObservers();
                        result = "Succeed";
                    }
                    catch (Exception e)
                    {
                        Form.showExceptionsInMessageBox(e.Message);
                        notifySearchPhotosObservers();
                    }
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Form.showExceptionsInMessageBox(ex.Message);
                notifySearchPhotosObservers();
            }

            return result;
        }

        public string Logout()  // on button Logout click
        {
            string result;
            try
            {
                FacebookService.Logout(null);
                FormMain nextForm = new FormMain();
                result = "Logout successfully";
                Form.Hide();
                nextForm.ShowDialog();
                Form.Dispose();
            }
            catch
            {
                result = "Logout failed";
            }

            return result;
        }

        private void getFilteredResults()
        {
            try
            {
                m_MostCommentablePhotosLogic.GetFilteredResults(
                r_LogicWrapper.LoggedInUser,
                ToBeSortedByIncreasingSort,
                FilteredResultsNotInARelationship,
                FilteredResultsGanderIsMale,
                FilteredResultsFromAge,
                FilteredResultsToAge,
                FilteredResultsQuantity,
                ref m_FilteredResult);
            }
            catch
            {
                throw;
            }
        }

        private void doubleCollectionLimit()
        {
            FacebookWrapper.FacebookService.s_CollectionLimit *= 2;
            r_LogicWrapper.LoginAndInit();
        }

        public void OlderEvents() // on button Older Events click
        {
            doubleCollectionLimit();
            GetEvents();
        }

        public void MorePages() // on button MorePages click
        {
            doubleCollectionLimit();
            GetPages();
        }

        public void MoreGroups() // on button MoreGroups click
        {
            doubleCollectionLimit();
            GetGroups();
        }

        public void MoreFriends() // on button MoreFriends click
        {
            doubleCollectionLimit();
            GetFriends();
        }
    }
}
