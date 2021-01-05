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
    public partial class FormApplication : Form
    {
        private FacadeApplication m_FacadeApplication;
        private List<ListViewItem> m_FilteredResult;

        public string FilteredResultsFromAge
        {
            get
            {
                return textBoxFromAge.Text.ToString();
            }

            private set { }
        }

        public string FilteredResultsToAge
        {
            get
            {
                return textBoxToAge.Text.ToString();
            }

            private set { }
        }

        public int FilteredResultsQuantity { get; private set; }

        public bool FilteredResultsNotInARelationship { get; private set; }

        public bool ToBeSortedByIncreasingSort { get; private set; }

        public bool FilteredResultsGanderIsMale { get; private set; }

        public FormApplication()
        {
            InitializeComponent();
            m_FacadeApplication = new FacadeApplication();
            m_FacadeApplication.Form = this;
            FacebookWrapper.FacebookService.s_CollectionLimit = 20;
            FacebookWrapper.FacebookService.s_FbApiVersion = 2.8f;
            m_FilteredResult = new List<ListViewItem>();
            radioButtonDecSort.Checked = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            attachInformation();
        }

        public void GetInformation()
        {
            paintButton(
                buttonLogout,
                buttonGetPosts,
                buttonOlderPosts,
                buttonGetEvents,
                buttonOlderEvents,
                buttonGetPages,
                buttonMorePages,
                buttonGetGroups,
                buttonMoreGroups,
                buttonGetFriends,
                buttonMoreFriends);

            makeVisibale(
                labelPostStatus,
                textBoxStatus,
                buttonSetStatus,
                pictureBoxProfile,
                labelUserName,
                pictureBoxPosts,
                labelPosts,
                labelEvents,
                pictureBoxEvents,
                labelPages,
                pictureBoxPages,
                labelGroups,
                pictureBoxGroups,
                labelFriends,
                pictureBoxFriends,
                radioButton1,
                radioButton3,
                radioButton5,
                radioButtonMale,
                radioButtonFemale);

            showBirthdayGreetingsFeature();
            showFilteredPictures();
            radioButton1.Select();
            radioButtonMale.Select();
        }

        private void paintButton(params Button[] i_Buttons)
        {
            foreach (Button btn in i_Buttons)
            {
                btn.BackColor = Color.FromArgb(66, 103, 178);
                btn.ForeColor = Color.White;
            }
        }

        private void makeVisibale(params Control[] i_Controls)
        {
            foreach (Control ctl in i_Controls)
            {
                ctl.Visible = true;
            }
        }

        private void attachInformation()
        {
            Thread collectBasicInformationThread = new Thread(() =>
            {
                m_FacadeApplication.GetProfilePictureAndUserName();
                this.Invoke(new Action(insertProfilePicture));
            });
            pictureBoxProfile.BackgroundImageLayout = ImageLayout.Stretch;
            collectBasicInformationThread.Start();
            facadeApplicationBindingSource.DataSource = m_FacadeApplication;
            postsBindingSource.DataSource = m_FacadeApplication.Posts;
            eventsBindingSource.DataSource = m_FacadeApplication.Events;
            pagesBindingSource.DataSource = m_FacadeApplication.Pages;
            groupsBindingSource.DataSource = m_FacadeApplication.Groups;
            friendsBindingSource.DataSource = m_FacadeApplication.Friends;
            friendsGreetingsBindingSource.DataSource = m_FacadeApplication.FriendsGreetings;
            friendsWhichHaveBirthdayBindingSource.DataSource = m_FacadeApplication.FriendsWhichHaveBirthday;
            filteredResultBindingSource.DataSource = m_FacadeApplication.FilteredResult;
            makeVisibale(buttonSearchPhotos);
            userPostsBindingSource.DataSource = m_FacadeApplication.UserPosts;
            m_FacadeApplication.PostsDelegates += new Action(() => this.Invoke(new Action(attachPostsBindingSource)));
            m_FacadeApplication.EventsDelegates += new Action(() => this.Invoke(new Action(attachEventsBindingSource)));
            m_FacadeApplication.GroupsDelegates += new Action(() => this.Invoke(new Action(attachGroupsBindingSource)));
            m_FacadeApplication.PagesDelegates += new Action(() => this.Invoke(new Action(attachPagesBindingSource)));
            m_FacadeApplication.FriendsDelegates += new Action(() => this.Invoke(new Action(attachFriendsBindingSource)));
            m_FacadeApplication.SearchPhotosDelegates += new Action(() => this.Invoke(new Action(attachSearchPhotosBindingSource)));
        }

        private void insertProfilePicture()
        {
            pictureBoxProfile.LoadAsync(m_FacadeApplication.ProfilePictureUrl);
        }

        private void showFilteredPictures()
        {
            makeVisibale(
                labelSecondFeatuer,
                labelQuantity,
                pictureBoxGander,
                labelAgeRange,
                textBoxFromAge,
                textBoxToAge,
                pictureBoxInRealation,
                checkBoxNotInARelationship,
                buttonSearchPhotos,
                filteredResultDataGridView);
        }

        private void showBirthdayGreetingsFeature()
        {
            makeVisibale(labelFirstFeature, buttonBirthdayGreetings, labelFriendsBirthdays, listBoxFriendsBirthdays, pictureBoxBirthdayFeatures, listBoxFriendsBirthdayGreetings);
        }

        private void buttonSetStatus_Click(object sender, EventArgs e)
        {
            MessageBox.Show(m_FacadeApplication.SetStatus(textBoxStatus.Text));
        }

        private void buttonBirthdayGreetings_Click(object sender, EventArgs e)
        {
            MessageBox.Show(m_FacadeApplication.BirthdayGreetings());
        }

        private void buttonSearchPhotos_Click(object sender, EventArgs e)
        {
            string result;
            changePressStatusOfSearchControllers();

            try
            {
                Thread thread = new Thread(() =>
                {
                    sendPropertiesToFacade();
                    result = m_FacadeApplication.SearchPhotos();
                    if (result != "Succeed")
                    {
                        MessageBox.Show(result);
                    }
                });
                thread.Start();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message + "here");
            }
        }

        private void changePressStatusOfSearchControllers()
        {
            buttonSearchPhotos.Visible = !buttonSearchPhotos.Visible;
            groupBox1.Enabled = !groupBox1.Enabled;
            radioButtonMale.Enabled = !radioButtonMale.Enabled;
            radioButtonFemale.Enabled = !radioButtonFemale.Enabled;
            textBoxFromAge.Enabled = !textBoxFromAge.Enabled;
            textBoxToAge.Enabled = !textBoxToAge.Enabled;
            checkBoxNotInARelationship.Enabled = !checkBoxNotInARelationship.Enabled;
        }

        private void attachSearchPhotosBindingSource()
        {
            filteredResultBindingSource.DataSource = m_FacadeApplication.FilteredResult;
            changePressStatusOfSearchControllers();
        }

        private void sendPropertiesToFacade()
        {
            m_FacadeApplication.FilteredResultsGanderIsMale = FilteredResultsGanderIsMale;
            m_FacadeApplication.FilteredResultsNotInARelationship = FilteredResultsNotInARelationship;
            m_FacadeApplication.ToBeSortedByIncreasingSort = ToBeSortedByIncreasingSort;
            m_FacadeApplication.FilteredResultsQuantity = FilteredResultsQuantity;
            m_FacadeApplication.FilteredResultsFromAge = FilteredResultsFromAge;
            m_FacadeApplication.FilteredResultsToAge = FilteredResultsToAge;
        }

        public void showExceptionsInMessageBox(string i_msg)
        {
            MessageBox.Show(i_msg);
        }

        private void textBoxFromAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.')
            {
                e.Handled = true;
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(m_FacadeApplication.Logout());
        }

        private void buttonGetPosts_Click(object sender, EventArgs e)
        {
            updatePosts();
        }

        private void updatePosts()
        {
            Thread thread = new Thread(() =>
            {
                m_FacadeApplication.GetPosts();
            });
            thread.Start();
        }

        private void attachPostsBindingSource()
        {
            postsBindingSource.DataSource = m_FacadeApplication.Posts;
            if (m_FacadeApplication.UserPosts.Count > 0)
            {
                userPostsBindingSource.DataSource = m_FacadeApplication.UserPosts[0];
            }

            makeVisibale(listBoxPosts, buttonOlderPosts);
        }

        private void buttonOlderPosts_Click(object sender, EventArgs e)  // to make the button unvisibale while thread is alive and visibale after thread is finished
        {
            Thread thread = new Thread(() =>
            {
                this.Invoke(new Action(m_FacadeApplication.OlderPosts));
            });
            thread.Start();
        }

        private void buttonGetEvents_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                m_FacadeApplication.GetEvents();
            });
            thread.Start();
        }

        private void attachEventsBindingSource()
        {
            eventsBindingSource.DataSource = m_FacadeApplication.Events;
            makeVisibale(listBoxEvents, buttonOlderEvents);
        }

        private void buttonOlderEvents_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                this.Invoke(new Action(m_FacadeApplication.OlderEvents));
            });
            thread.Start();
        }

        private void buttonGetPages_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                m_FacadeApplication.GetPages();
            });
            thread.Start();
        }

        private void attachPagesBindingSource()
        {
            pagesBindingSource.DataSource = m_FacadeApplication.Pages;
            makeVisibale(listBoxPages, buttonMorePages);
        }

        private void buttonMorePages_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                this.Invoke(new Action(m_FacadeApplication.MorePages));
            });
            thread.Start();
        }

        private void buttonGetGroups_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                m_FacadeApplication.GetGroups();
            });
            thread.Start();
        }

        private void attachGroupsBindingSource()
        {
            groupsBindingSource.DataSource = m_FacadeApplication.Groups;
            makeVisibale(listBoxGroups, buttonMoreGroups);
        }

        private void buttonMoreGroups_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                this.Invoke(new Action(m_FacadeApplication.MoreGroups));
            });
            thread.Start();
        }

        private void buttonGetFriends_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                m_FacadeApplication.GetFriends();
            });
            thread.Start();
        }

        private void attachFriendsBindingSource()
        {
            friendsBindingSource.DataSource = m_FacadeApplication.Friends;
            makeVisibale(listBoxFriends, buttonMoreFriends);
        }

        private void buttonMoreFriends_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                this.Invoke(new Action(m_FacadeApplication.MoreFriends));
            });
            thread.Start();
        }

        private void radioButtonMale_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonMale.Checked)
            {
                FilteredResultsGanderIsMale = true;
            }
        }

        private void radioButtonFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFemale.Checked)
            {
                FilteredResultsGanderIsMale = false;
            }
        }

        private void checkBoxNotInARelationship_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNotInARelationship.Checked)
            {
                FilteredResultsNotInARelationship = true;
            }
            else
            {
                FilteredResultsNotInARelationship = false;
            }
        }

        private void groupBox1_Validating(object sender, CancelEventArgs e)
        {
            if (radioButton1.Checked)
            {
                FilteredResultsQuantity = 1;
            }
            else if (radioButton3.Checked)
            {
                FilteredResultsQuantity = 3;
            }
            else if (radioButton5.Checked)
            {
                FilteredResultsQuantity = 5;
            }
        }

        private void listBoxPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxPosts.SelectedIndex != 0)
            {
                userPostsBindingSource.DataSource = m_FacadeApplication.UserPosts[listBoxPosts.SelectedIndex];
            }
        }

        private void messageTextBox_Validating(object sender, CancelEventArgs e)
        {
            updatePosts();
            userPostsBindingSource.DataSource = m_FacadeApplication.UserPosts[listBoxPosts.SelectedIndex];
        }

        private void FormApplication_Load(object sender, EventArgs e)
        {
        }

        private void radioButtonIncrSort_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonIncSort.Checked)
            {
                ToBeSortedByIncreasingSort = true;
            }
        }

        private void radioButtonDecSort_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDecSort.Checked)
            {
                ToBeSortedByIncreasingSort = false;
            }
        }
    }
}
