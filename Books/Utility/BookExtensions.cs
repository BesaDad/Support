using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Books.Domain.Models;

namespace Books.Utility
{
    public static class BookExtensions
    {
        public static List<Book> GetSortetList(this List<Book> books, string orderProp, string orderType)
        {
            switch (orderProp)
            {
                case "PublishYear":
                    return !string.IsNullOrEmpty(orderType) ? books.OrderBy(it => it.PublishYear).ToList() : books.OrderByDescending(it => it.PublishYear).ToList();
                case "Tittle":
                    return !string.IsNullOrEmpty(orderType) ? books.OrderBy(it => it.Tittle).ToList() : books.OrderByDescending(it => it.Tittle).ToList();
                default:
                    return !string.IsNullOrEmpty(orderType) ? books.OrderBy(it => it.Id).ToList() : books.OrderByDescending(it => it.Id).ToList();
            }
        }
    }
}