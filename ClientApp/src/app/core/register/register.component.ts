import { Component } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { RegisterModel } from '../../models/RegisterModel';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
})
export class RegisterComponent {
  registerForm: FormGroup;

  errorMessage: string | null = null;
  successMessage: string | null = null;
  isLoading: boolean = false;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;  // Включить спиннер

    const registerModel = new RegisterModel(
      this.registerForm.value.firstName,
      this.registerForm.value.lastName,
      this.registerForm.value.email,
      this.registerForm.value.password
    );

    this.authService.register(registerModel).subscribe(
      (response) => {
        const token = response.token;
        this.authService.saveToken(token);
        //localStorage.setItem('authToken', token);
        console.log("Регистрация пользователя прошла успешно", response);
        this.successMessage = "Регистрация успешна! Теперь вы можете войти.";
        this.errorMessage = null;
        this.isLoading = false; //   Выключить спиннер

        // Очистить форму, если нужно
        //this.registerForm.reset();

        // Перенаправить на страницу входа через небольшую задержку
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      (error) => {
        console.error("Регистрация не удалась", error);
        this.successMessage = null;
        this.isLoading = false;

        // Обработка ошибки с выводом сообщения
        if (error.error && error.error.message) {
          this.errorMessage = `Ошибка: ${error.error.message}`;
        } else {
          this.errorMessage = "Что-то пошло не так. Попробуйте еще раз.";
        }
      }
    );
  }


}
