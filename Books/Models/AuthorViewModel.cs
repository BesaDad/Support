using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Books.Models
{
    public class AuthorViewModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Фамилия автора")]
        public string FirstName { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя автора")]
        public string LastName { get; set; }

        public BookViewModel Book { get; set; }
    }
}