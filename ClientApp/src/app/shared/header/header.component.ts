import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [RouterModule, CommonModule]
})
export class HeaderComponent implements OnInit {
  currentUser: any = null;  // текущий пользователь

  constructor(private authService: AuthService, private router: Router) { };

  ngOnInit(): void {
    // подписываемся на изменения состояния пользователя
    this.authService.getCurrentUserObservable().subscribe((user) => {
      this.currentUser = user;
    });
  }

  logout(): void {
    //console.log("в хедере вызываем через сервис AuthService");
    this.authService.logout();
    //window.location.reload(); // перезагружаем страницу для обновления хедера
    this.router.navigate(['/login']);
  }
}
