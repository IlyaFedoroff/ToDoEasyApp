import { Component, OnInit } from '@angular/core';
import { TodoService } from './todo.service'; // Импортируем TodoService
import { TodoItem } from './todo-item.model'; // Импортируем TodoItem
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports:[RouterModule, CommonModule]
})
export class AppComponent {
  //todoItems: TodoItem[] = [];
  //newTodoItem: TodoItem = { id: 0, title: '', isCompleted: false };

  //constructor(private todoService: TodoService) { }

  //ngOnInit(): void {
  //  this.loadTodos();
  //}

  //loadTodos(): void {
  //  this.todoService.getTodoItems().subscribe((data) => {
  //    this.todoItems = data;
  //  });
  //}

  //addTodo(): void {
  //  if (this.newTodoItem.title) {
  //    this.todoService.addTodoItem(this.newTodoItem).subscribe((todo) => {
  //      this.todoItems.push(todo);
  //      this.newTodoItem = { id: 0, title: '', isCompleted: false }; // Очищаем форму
  //    });
  //  }
  //}

  //deleteTodo(id: number): void {
  //  this.todoService.deleteTodoItem(id).subscribe(() => {
  //    this.todoItems = this.todoItems.filter(todo => todo.id !== id);
  //  });
  //}

  //updateTodo(todo: TodoItem): void {
  //  this.todoService.updateTodoItem(todo.id, todo).subscribe();
  //}
}
