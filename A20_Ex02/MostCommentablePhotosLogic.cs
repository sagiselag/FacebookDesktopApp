using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public class MostCommentablePhotosLogic
    {
        Wrapper LogicWrapper = Wrapper.Instace;

        public FacebookObjectCollection<Photo> Photos { get; private set; }

        public FacebookObjectCollection<Album> Albums { get; private set; }

        private Func<int, int, bool> ComparerMethod { get; set; }

        private void getAllPhotos(User i_LoggedInUser)
        {
            Albums = i_LoggedInUser.Albums;
            Photos = SingletonPhotos.Photos(Albums);
        }

        private void fetchMostOrMinCommentablePhotos(User i_LoggedInUser, string i_Gender, int i_Quantity, int i_MinAge, int i_MaxAge, bool i_NotInARealationshipFilter, ref List<TargetedPhotoInformation> io_ListOfPicturesData)
        {
            FacebookObjectCollection<Comment> commentByUsers;
            List<TargetedPhotoInformation> listOfAllPicturesData = new List<TargetedPhotoInformation>();
            List<string> photoData = new List<string>();
            int counterOfCommentsFromTargetAudiens;
            string photoLink;
            int totalComments;
            int userAge;
            string userGender;
            bool meetRelationshipFilter;
            UserLogic loggedInUser = new UserLogic(i_LoggedInUser);
            User user;

            getAllPhotos(loggedInUser.User);
            foreach (Photo photo in Photos)
            {
                commentByUsers = photo.Comments;
                counterOfCommentsFromTargetAudiens = 0;
                meetRelationshipFilter = true;
                photoLink = photo.Link;
                totalComments = commentByUsers.Count;

                foreach (Comment comment in commentByUsers)
                {
                    user = comment.From;
                    if (i_NotInARealationshipFilter == true)
                    {
                        try
                        {
                            if (LogicWrapper.IsInARelationship(user))
                            {
                                meetRelationshipFilter = false;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }

                    if (meetRelationshipFilter == true)
                    {
                        userAge = loggedInUser.Age;
                        userGender = loggedInUser.Gander;

                        if (userAge >= i_MinAge && userAge <= i_MaxAge)
                        {
                            if (userGender == i_Gender)
                            {
                                counterOfCommentsFromTargetAudiens++;
                            }
                        }
                    }
                }

                listOfAllPicturesData.Add(new TargetedPhotoInformation(photoLink, totalComments, counterOfCommentsFromTargetAudiens, ComparerMethod));
            }

            if (listOfAllPicturesData.Count > 0)
            {
                listOfAllPicturesData.Sort();
            }

            if (listOfAllPicturesData.Count > i_Quantity)
            {
                io_ListOfPicturesData.AddRange(listOfAllPicturesData.GetRange(0, i_Quantity));
            }
            else
            {
                io_ListOfPicturesData = listOfAllPicturesData;
            }
        }

        public void GetFilteredResults(User i_User, bool v_ToBeSortedByIncreasingSort, bool i_isNotInARealationship, bool i_IsMale, string i_MinAge, string i_MaxAge, int i_Quantity, ref List<TargetedPhotoInformation> i_FilteredResults)
        {
            List<TargetedPhotoInformation> filteredResults = new List<TargetedPhotoInformation>();
            List<TargetedPhotoInformation> pictureData = new List<TargetedPhotoInformation>();
            string gender;
            string totalComments, relevantLikes;
            int minAge, maxAge;

            if (v_ToBeSortedByIncreasingSort == true)
            {
                ComparerMethod = (currPhoto, otherPhoto) => currPhoto > otherPhoto;
            }
            else
            {
                ComparerMethod = (currPhoto, otherPhoto) => currPhoto < otherPhoto;
            }


            gender = i_IsMale ? "male" : "female";
            try
            {
                maxAge = int.Parse(i_MaxAge);
                minAge = int.Parse(i_MinAge);
                if (maxAge >= minAge)
                {
                    try
                    {
                        fetchMostOrMinCommentablePhotos(i_User, gender, i_Quantity, minAge, maxAge, i_isNotInARealationship, ref pictureData);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(e.Message);
                    }

                    foreach (TargetedPhotoInformation item in pictureData)
                    {
                        totalComments = item.TotalComments.ToString();
                        relevantLikes = item.CommentsFromTargetAudiens.ToString();
                        filteredResults.Add(item);
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("Maximum age must be bigger than Minimum age!!");
                }

                i_FilteredResults = filteredResults;
            }
            catch (InvalidOperationException iOE)
            {
                throw new Exception(iOE.Message);
            }
            catch (IndexOutOfRangeException iOORE)
            {
                throw new Exception(iOORE.Message);
            }
            catch
            {
                throw new Exception("only numbers please!!");
            }
        }
    }
}