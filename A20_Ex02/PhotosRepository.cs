using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public sealed class SingletonPhotos
    {
        private static FacebookObjectCollection<Photo> s_Photos = null;
        private static readonly object sr_Lock = new object();

        private SingletonPhotos(FacebookObjectCollection<Album> albums)
        {
            FacebookObjectCollection<Photo> albumPhotos;
            s_Photos = new FacebookObjectCollection<Photo>();

            foreach (Album album in albums)
            {
                try
                {
                    albumPhotos = album.Photos;
                    foreach (Photo photo in albumPhotos)
                    {
                        s_Photos.Add(photo);
                    }
                }
                catch
                {
                    s_Photos = new FacebookObjectCollection<Photo>();
                }
            }
        }

        public static FacebookObjectCollection<Photo> Photos(FacebookObjectCollection<Album> albums)
        {
            if (s_Photos == null || newPictureAdded(albums) == true)
            {
                lock (sr_Lock)
                {
                    if (s_Photos == null || newPictureAdded(albums) == true)
                    {
                        new SingletonPhotos(albums);
                    }
                }
            }

            return s_Photos;
        }

        private static Boolean newPictureAdded(FacebookObjectCollection<Album> albums)
        {
            int photosCounter = 0;
            try
            {
                foreach (Album album in albums)
                {
                    photosCounter += album.Photos.Count;
                }

                return s_Photos.Count == photosCounter;
            }
            catch
            {
                // if can't access photos counter get them again because you can't know if there are new photos
                return true;
            }
        }
    }
}
