import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { FormGroup, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { LoginModel } from '../../models/LoginModel';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onLogin(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.errorMessage = null;

    const loginModel = new LoginModel(
      this.loginForm.value.email,
      this.loginForm.value.password
    );

    this.authService.login(loginModel).subscribe({
      next: (response: any) => {
        console.log("Пользователь успешно авторизовался", response);

        // получаем данные пользователя
        const userData = this.authService.getCurrentUser();
        this.authService.setCurrentUser(userData);  // сохраняем данные пользователя

        this.router.navigate(['/todos']); //   перенаправление на защищенную страницу
      },
      error: (error) => {
        console.error("Авторизация не удалась", error);

        // извлекаем сообщение об ошибке из ответа сервера
        if (error.error && error.error.message) {
          this.errorMessage = error.error.message;  // сохраняем сообщение об ошибке
        } else {
          this.errorMessage = "Произошла ошибка при авторизации. Попробуйте снова.";
        }
      },
    });
  }
}
