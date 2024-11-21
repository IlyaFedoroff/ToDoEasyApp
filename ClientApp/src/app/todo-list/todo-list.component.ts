// src/app/todo-list/todo-list.component.ts
import { Component, OnInit } from '@angular/core';
import { TodoService } from '../todo.service';
import { TodoItem } from '../todo-item.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class TodoListComponent implements OnInit {
  searchActive: boolean = false;
  filterText: string = '';
  todoItems: TodoItem[] = [];
  newTodoTitle: string = '';

  constructor(private todoService: TodoService) { }

  ngOnInit(): void {
    this.loadTodoItems();
  }

  loadTodoItems() {
    this.todoService.getTodoItems().subscribe(
      (data) => {
        this.todoItems = data;
      },
      (error) => {
        console.error("Ошибка загрузки данных", error);
      }
    );
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
    if (this.newTodoTitle.trim()) {
      const newTodo: TodoItem = {
        id: Math.floor(Math.random() * 2147483647), // Генерация числа в диапазоне int
        title: this.newTodoTitle,
        isCompleted: false
      };

      this.todoItems.push(newTodo);

      console.log("Sending new todo item: ", newTodo);

      this.todoService.addTodoItem(newTodo).subscribe(
        (data) => {
          console.log("Todo item added", data);
          this.loadTodoItems();
        },
        (error) => {
          console.error('Error adding todo item', error);
          console.error("todo item looks: ", newTodo);
          if (error.error) {
            console.log("Error details:", error.error);
          }
        }
      );

      this.newTodoTitle = '';
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
    this.todoService.updateTodoItem(todo.id, todo).subscribe(() => {
      todo.isEditing = false;
    });
  }

  cancelEdit(todo: TodoItem): void {
    todo.isEditing = false;
    this.loadTodoItems();
  }

  updateTodoCompletion(todo: TodoItem): void {
    this.todoService.updateTodoItem(todo.id, todo).subscribe(
      () => {
        console.log("Todo item updated:", todo);
      },
      (error) => {
        console.error("Error updating todo item", error);
      }
    );
  }

}
