import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpEvent,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';


export const AuthInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {

  const jwtHelper = new JwtHelperService();

  const token = localStorage.getItem('authToken');
    //console.log("Токен получен в интерцепторе:", token);
    //console.log("Исходный запрос:", req);

    if (token && !jwtHelper.isTokenExpired(token)) {
      const cloned = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });

      //console.log("Запрос с добавленным токеном", cloned);

      return next(cloned);
  }

    return next(req);
}
