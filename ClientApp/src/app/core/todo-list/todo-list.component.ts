// src/app/todo-list/todo-list.component.ts
import { Component, OnInit } from '@angular/core';
import { TodoService } from '../../services/todo/todo.service';
import { TodoItem } from '../../models/todo-item.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { TodoType } from '../../models/todoType';

@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, MatSnackBarModule]
})
export class TodoListComponent implements OnInit {
  searchActive: boolean = false;
  filterText: string = '';
  todoItems: TodoItem[] = [];
  newTodoTitle: string = '';
  errorMessage: string | null = null;
  selectedTypeId: number | null = null; // выбранный тип задачи
  taskTypes: TodoType[] = [];

  constructor(private todoService: TodoService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadTodoItems();
    this.loadTaskTypes();
  }

  loadTodoItems() {
    this.todoService.getTodoItems().subscribe({
      next: (data) => {
        this.todoItems = data;
      },
      error: (error) => {
        console.error("Ошибка загрузки данных", error);
        this.errorMessage = "Не удалось загрузить данные задачи.";
      },
      complete: () => {
        // логика которая выполняется после завершения потока
      }
    });
  }

  loadTaskTypes(): void {
    this.todoService.getTaskTypes().subscribe(
      (types) => {
        this.taskTypes = types;
      },
      (error) => {
        this.snackBar.open('Ошибка при загрузке типов задач', 'Закрыть', { duration: 3000 });
      }
    )
  }

  get filteredTodoItems() {
    if (this.searchActive && this.filterText) {
      return this.todoItems.filter(todo =>
        todo.title.toLowerCase().includes(this.filterText.toLowerCase())
      );
    } else {
      return this.todoItems;
    }
  }

  toggleSearch() {
    this.searchActive = !this.searchActive;
    if (!this.searchActive) {
      this.filterText = '';
    }
  }

  addTodo() {
    if (this.newTodoTitle.trim() && this.selectedTypeId !== null) {
      const newTodo: TodoItem = {
        title: this.newTodoTitle,
        isCompleted: false,
        typeId: this.selectedTypeId
      };

      this.todoItems.push(newTodo);

      this.todoService.addTodoItem(newTodo).subscribe({
        next: (createdTodo) => {
          this.snackBar.open('Задача успешно добавлена', 'Закрыть', { duration: 3000 });
          this.loadTodoItems();
        },
        error: (error) => {
          this.snackBar.open('Ошибка при добавлении задачи', 'Закрыть', { duration: 3000 });
          const index = this.todoItems.indexOf(newTodo);
          if (index !== -1) {
            this.todoItems.splice(index, 1);
          }
        }
      });

      this.newTodoTitle = '';
      this.selectedTypeId = null;
    } else {
      this.snackBar.open('Заполните название и выберите тип задачи', 'Закрыть', { duration: 3000 });
    }
  }


  deleteTodoItem(id: number) {
    this.todoService.deleteTodoItem(id).subscribe(() => {
      this.loadTodoItems();
    });
  }

  editTodoItem(todo: TodoItem): void {
    todo.isEditing = true;
  }

  saveTodoItem(todo: TodoItem): void {
    if (todo.id == undefined) {
      console.error('ID is undefined. Cannot save todo item.');
      return;
    }

    this.todoService.updateTodoItem(todo.id, todo).subscribe(() => {
      todo.isEditing = false;
    });
  }

  cancelEdit(todo: TodoItem): void {
    todo.isEditing = false;
    this.loadTodoItems();
  }

  updateTodoCompletion(todo: TodoItem): void {

    if (todo.id == undefined) {
      console.error('ID is undefined. Cannot update todo item.');
      return;
    }

    if (todo.typeId == null) {
      this.snackBar.open('Выберите тип задачи', 'Закрыть', { duration: 3000 });
      return;
    }

    this.todoService.updateTodoItem(todo.id, todo).subscribe({
      next: () => {
        todo.isEditing = false;
        this.snackBar.open('Задача успешно обновлена', 'Закрыть', { duration: 3000 });
    },
      error: (error) => {
        this.snackBar.open('Ошибка при обновлении задачи', 'Закрыть', { duration: 3000 });
        console.error("Error updating todo item", error);
    },
    complete: () => {
        // логика при завершении
      }
    });
  }

}
