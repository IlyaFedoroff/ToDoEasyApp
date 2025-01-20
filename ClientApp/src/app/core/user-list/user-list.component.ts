import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user/user.service';
import { UserWithCompletedTodosDto } from '../../models/UserWithCompletedTodosDto';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css',
  standalone: true,
  imports: [CommonModule]
})
export class UserListComponent implements OnInit {
  users: UserWithCompletedTodosDto[] = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.getUsersSortedByCompletedTodos();
  }

  getUsersSortedByCompletedTodos(): void {
    this.userService.getUsersSortedByCompletedTodos().subscribe(
      (data) => {
        this.users = data;
      },
      (error) => {
        console.error('Ошибка при получении пользователей: ', error);
      }
    );
  }
}
