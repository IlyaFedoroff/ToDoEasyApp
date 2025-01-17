// src/app/todo.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoItem } from '../../models/todo-item.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';


@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private apiUrl = 'https://localhost:7073/api/todoitems';

  constructor(private http: HttpClient, private authService: AuthService) { }

  //const token = this.authService.getToken();
  //const headers = new HttpHeaders().set(`Authorization`, `Bearer ${token}`);


  getTodoItems(): Observable<TodoItem[]> {
   
    return this.http.get<TodoItem[]>(this.apiUrl);
  }

  getTodoItem(id: number): Observable<TodoItem> {

    return this.http.get<TodoItem>(`${this.apiUrl}/${id}`);
  }

  addTodoItem(todoItem: TodoItem): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.apiUrl, todoItem);
    }

  updateTodoItem(id: number, todoItem: TodoItem): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, todoItem);
  }

  deleteTodoItem(id: number): Observable<void> {
    console.log("удаляем todoitem с id:", id);
    console.log("URL запроса:", `${this.apiUrl}/${id}`);
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
