import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserWithCompletedTodosDto } from '../../models/UserWithCompletedTodosDto';
import { User } from '../../models/user.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) { }

  // получение пользователей, отсортированных по количеству выполненных задач
  getUsersSortedByCompletedTodos(): Observable<UserWithCompletedTodosDto[]> {
    return this.http.get<UserWithCompletedTodosDto[]>(`${this.apiUrl}/sorted-by-completed-todos`);
  }

  // получение пользователей, отсортированных по недавней активности
  getUsersSortedByRecentActivity(): Observable<UserWithCompletedTodosDto[]> {
    return this.http.get<UserWithCompletedTodosDto[]>(`${this.apiUrl}/sorted-by-recent-activity`);
  }

  // получение пользователей, отсортированных по разнице выполненных и незавершенных задач
  getUsersSortedByTaskDifference(): Observable<UserWithCompletedTodosDto[]> {
    return this.http.get<UserWithCompletedTodosDto[]>(`${this.apiUrl}/sorted-by-task-difference`);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}`);
  }

}
