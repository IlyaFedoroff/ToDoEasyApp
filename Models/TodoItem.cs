﻿using System.ComponentModel.DataAnnotations;
using ToDoEasyApp.Models.Validations;

namespace ToDoEasyApp.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            IsCompleted = false;    //   значение по умолчанию
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [TodoItem_EnsureTitleIsCorrect]
        public string? Title { get; set; }

        [Required]
        public bool? IsCompleted { get; set; }

    }
}
