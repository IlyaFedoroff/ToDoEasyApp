import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterModel } from '../../models/RegisterModel';
import { LoginModel } from '../../models/LoginModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {


  private apiUrl = 'https://localhost:7073/api/auth';

  constructor(private http: HttpClient) { }

  register(registerModel: RegisterModel): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, registerModel);
  }

  login(loginModel: LoginModel): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, loginModel);
  }

  logout(): void {

    localStorage.removeItem('token');
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

}
