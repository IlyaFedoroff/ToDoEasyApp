import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/core/main/app.component';
import { routes } from './app/app-routing.module';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthInterceptor } from './app/features/authinter/auth.interceptor';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';


bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideAnimations(),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([AuthInterceptor])),  // Подключаем HTTP-сервис
  ]
}).catch((err) => console.error(err));
