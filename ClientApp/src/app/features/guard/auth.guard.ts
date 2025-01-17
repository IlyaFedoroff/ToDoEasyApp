import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';


export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);  // внедряем AuthService
  const router = inject(Router);  // внедряем Router
  const snackBar = inject(MatSnackBar); // внедряем MatSnackBar

  if (authService.isAuthenticated()) {
    return true;  // разрешить доступ
  } else {
    // показываем уведомление
    snackBar.open('Для доступа к этой странице необходимо войти в систему.', 'Закрыть', {
      duration: 3000,
    });

    return router.createUrlTree(['/login']);  // перенаправить на страницу входа
  }
};
