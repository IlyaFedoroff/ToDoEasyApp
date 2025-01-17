import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterModel } from '../../models/RegisterModel';
import { LoginModel } from '../../models/LoginModel';
import { JwtHelperService } from '@auth0/angular-jwt';
import { tap } from 'rxjs/operators'
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private jwtHelper = new JwtHelperService();

  private apiUrl = 'https://localhost:7073/api/auth';

  private tokenKey = 'authToken';

  private currentUserSubject = new BehaviorSubject<any>(null);  // отслеживаем состояние пользователя

  constructor(private http: HttpClient)
  {
    this.currentUserSubject.next(this.getCurrentUser());  // инициализируем состояние
  }


  // сохранение токена и данных пользователя
  setCurrentUser(user: any): void {
    console.log('Сохранение данных пользователя:', user);
    this.currentUserSubject.next(user);   // обновляем состояние
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  // получение данных пользователя
  getCurrentUser(): any {
    const token = this.getToken();
    if (token) {
      return this.decodeToken(token);
    }
    return null;
  }

  register(registerModel: RegisterModel): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/register`, registerModel);
  }

  login(loginModel: LoginModel): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, loginModel).pipe(
      tap(response => {
        this.saveToken(response.token);
      })
    )

  }

  logout(): void {
    this.currentUserSubject.next(null); // сбрасываем состояние
    localStorage.removeItem('currentUser');
    this.removeToken();
  }

  // Получение Observable для текущего пользователя
  getCurrentUserObservable() {
    return this.currentUserSubject.asObservable();
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) {
      return false; // Токен отсутствует
    }
    return !this.jwtHelper.isTokenExpired(token); // Проверяем, истек ли срок действия
  }

  // tokens

  // декодирование токена
  decodeToken(token: string): any {
    console.log("Декодирование токена: ", token);
    const decodedToken = this.jwtHelper.decodeToken(token);
    console.log("Декодированный токен: ", decodedToken);
    return decodedToken;
  }

  saveToken(token: string) {
    console.log("Токен сохранен:", token);
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  removeToken() {
    localStorage.removeItem(this.tokenKey);
  }

}
