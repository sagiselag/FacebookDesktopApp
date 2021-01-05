using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A20_Ex01
{
    public class TargetedPhotoInformation : IComparable
    {
        protected Func<int, int, bool> ComparerMethod { get; set; }

        public string Link { get; set; }

        public int TotalComments { get; set; }

        public int CommentsFromTargetAudiens { get; set; }

        public TargetedPhotoInformation(string i_Link, int i_TotalComments, int i_CommentsFromTargetAudiens, Func<int, int, bool> i_ComparerMethod)
        {
            Link = i_Link;
            TotalComments = i_TotalComments;
            CommentsFromTargetAudiens = i_CommentsFromTargetAudiens;
            ComparerMethod = i_ComparerMethod;
        }

        public int CompareTo(object obj)
        {
            int otherPhotoCommentsFromTargetAudiens = ((TargetedPhotoInformation)obj).CommentsFromTargetAudiens;
            int otherPhotoTotalComments = ((TargetedPhotoInformation)obj).TotalComments;
            int result = 0;

            if (ComparerMethod.Invoke(CommentsFromTargetAudiens, otherPhotoCommentsFromTargetAudiens) == true)
            {
                result = 1;
            }
            else if (CommentsFromTargetAudiens != otherPhotoCommentsFromTargetAudiens)
            {
                result = -1;
            }
            else
            {
                if (ComparerMethod.Invoke(TotalComments, otherPhotoTotalComments) == true)
                {
                    result = 1;
                }
                else if (TotalComments != otherPhotoTotalComments)
                {
                    result = -1;
                }
            }
            
            return result;
        }
    }
}
