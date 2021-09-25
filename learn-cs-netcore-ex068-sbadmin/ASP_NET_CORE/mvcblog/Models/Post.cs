using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcblog.Models
{
    public interface PostPrototype
    {
        PostPrototype Clone();
    }

    [Table("Post")]
    public class Post : PostBase, PostPrototype
    {

        [Required]
        [Display(Name = "Tác giả")]
        public string AuthorId {set; get;}
        [ForeignKey("AuthorId")]
        [Display(Name = "Tác giả")]
        public AppUser Author {set; get;}
     
        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated {set; get;}

        [Display(Name = "Ngày cập nhật")]
        public DateTime DateUpdated {set; get;}

        public PostPrototype Clone()
        {
            // Khong clone Post ID
            Post newPost = new Post();
            newPost.Title = Title;
            newPost.Description = Description;
            newPost.Slug = Slug;
            newPost.Content = Content;
            newPost.Published = Published;
            newPost.AuthorId = AuthorId;
            newPost.DateCreated = DateCreated;
            newPost.DateUpdated = DateUpdated;
            return newPost;
        }
    }
}