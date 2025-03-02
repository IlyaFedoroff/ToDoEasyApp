// src/app/todo.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoItem } from '../../models/todo-item.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { TodoType } from '../../models/todoType';
import { TodoItemForSearchDto } from '../../models/TodoItemForSearchDto';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private apiUrl = `${environment.apiUrl}/TodoItems`;

  constructor(private http: HttpClient, private authService: AuthService) { }

  //const token = this.authService.getToken();
  //const headers = new HttpHeaders().set(`Authorization`, `Bearer ${token}`);

  getTaskTypes(): Observable<TodoType[]> {
    return this.http.get<TodoType[]>(`${this.apiUrl}/types`);
  }

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

  searchTodos(createdAt: Date | null, typeId: number | null, authorId: string | null): Observable<TodoItemForSearchDto[]> {

    const params: any = {};

    if (createdAt) {
      //console.log("createdAt:", createdAt);
      params.createdAt = createdAt;
    }
    if (typeId !== null) {
      params.typeId = typeId;
    }
    if (authorId) {
      params.authorId = authorId;
    }


    return this.http.get<TodoItemForSearchDto[]>(`${this.apiUrl}/search-dapper`, { params });
  }

}
