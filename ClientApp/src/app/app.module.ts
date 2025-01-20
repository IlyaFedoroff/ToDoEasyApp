import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './core/main/app.component';


import { TodoListComponent } from './core/todo-list/todo-list.component';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';
import { HomeComponent } from './core/home/home.component';
import { AboutComponent } from './core/about/about.component';
import { ContactComponent } from './core/contact/contact.component';
import { RegisterComponent } from './core/register/register.component';
import { LoginComponent } from './core/login/login.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './features/authinter/auth.interceptor';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { SearchTodoComponent } from './core/search-todo/search-todo.component';
//import { UserListComponent } from './core/user-list/user-list.component';


@NgModule({
  declarations: [
    AppComponent,
    TodoListComponent,
    HeaderComponent,
    FooterComponent,
    HomeComponent,
    AboutComponent,
    ContactComponent,
    RegisterComponent,
    LoginComponent,
    SearchTodoComponent
    //UserListComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    CommonModule,
    BrowserAnimationsModule,
  ],
  providers: [
    provideHttpClient(
      withInterceptors([AuthInterceptor])
    ),
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor() {
    console.log("AppModule Загружен и интерцептор зарегестрирован!");
  }
}
