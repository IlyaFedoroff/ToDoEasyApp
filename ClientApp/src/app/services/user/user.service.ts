import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserWithCompletedTodosDto } from '../../models/UserWithCompletedTodosDto';
import { User } from '../../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'https://localhost:7073/api/users';

  constructor(private http: HttpClient) { }

  getUsersSortedByCompletedTodos(): Observable<UserWithCompletedTodosDto[]> {
    return this.http.get<UserWithCompletedTodosDto[]>(`${this.apiUrl}/sorted-by-completed-todos`);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}`);
  }

}
