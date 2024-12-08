// src/app/todo.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoItem } from '../../models/todo-item.model';
import { HttpClient } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private apiUrl = 'https://localhost:7073/api/todoitems';

  constructor(private http: HttpClient) { }



  getTodoItems(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.apiUrl);
  }

  getTodoItem(id: number): Observable<TodoItem> {
    return this.http.get<TodoItem>(`${this.apiUrl}/${id}`);
  }

  addTodoItem(todoItem: TodoItem): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.apiUrl, todoItem, {
      headers: {'Content-Type': 'application/json'}
    });
  }

  updateTodoItem(id: number, todoItem: TodoItem): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, todoItem);
  }

  deleteTodoItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
