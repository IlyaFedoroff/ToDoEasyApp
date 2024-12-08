import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/core/main/app.component';
import { routes } from './app/app-routing.module';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),  // Подключаем HTTP-сервис
  ]
}).catch((err) => console.error(err));
