using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Foolproof;

namespace Books.Models
{
    public class BookViewModel
    {
        public BookViewModel()
        {
            Authors = new List<AuthorViewModel>();
        }
        public int Id { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Заголовок")]
        public string Tittle { get; set; }

        [Range(1, 10000)]
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Количество страниц")]
        public int PageCount { get; set; }

        [MaxLength(30)]
        [Display(Name = "Название издательства")]
        public string PublishName { get; set; }

        [Range(1800, int.MaxValue)]
        [Display(Name = "Год публикации")]
        public int? PublishYear { get; set; }

        [Display(Name = "Номер ISBN")]
        public string ISBN { get; set; }

        [Display(Name = "Изображение")]
        public string ImagePath { get; set; }

        [Display(Name = "Авторы")]
        public virtual List<AuthorViewModel> Authors { get; set; }
    }
}