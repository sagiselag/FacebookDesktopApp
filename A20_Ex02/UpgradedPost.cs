using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public class UpgradedPost
    {
        public string Message { get; set; }

        private Post m_Post;

        public string Link
        {
            get
            {
                return m_Post.Link;
            }

            private set { }
        }

        public DateTime? UpdateTime
        {
            get
            {
                return m_Post.UpdateTime;
            }

            private set { }
        }

        public DateTime? CreatedTime
        {
            get
            {
                return m_Post.CreatedTime;
            }

            private set { }
        }

        public string Caption
        {
            get
            {
                return m_Post.Caption;
            }

            private set { }
        }

        public Post.eType? Type
        {
            get
            {
                return m_Post.Type;
            }

            private set { }
        }

        public UpgradedPost(Post i_Post)
        {
            m_Post = i_Post;
            Message = i_Post.Message;
        }
    }
}
