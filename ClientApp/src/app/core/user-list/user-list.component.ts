import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../services/user/user.service';
import { UserWithCompletedTodosDto } from '../../models/UserWithCompletedTodosDto';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css',
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class UserListComponent implements OnInit {
  users: UserWithCompletedTodosDto[] = [];
  selectedSortMode: 'completedTasks' | 'recentActivity' | 'taskDifference' = 'recentActivity';

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.onSortModeChange();
  }

  onSortModeChange(): void {
    switch (this.selectedSortMode) {
      case 'completedTasks':
        this.getUsersSortedByCompletedTodos();
        break;
      case 'recentActivity':
        this.getUsersSortedByRecentActivity();
        break;
      case 'taskDifference':
        this.getUsersSortedByTaskDifference();
    }
  }

  // по количеству выполненных задач
  getUsersSortedByCompletedTodos(): void {
    this.userService.getUsersSortedByCompletedTodos().subscribe({
      next: (d) => {
        console.log('Получены пользователи:', d);
        this.users = d;
      },
      error: (e) => {
        console.error('Ошибка при получении пользователей: ', e);
      }
  });
  }

  // по недавней активности
  getUsersSortedByRecentActivity(): void {
    this.userService.getUsersSortedByRecentActivity().subscribe({
      next: (d) => {
        console.log('Получены пользователи:', d);
        this.users = d;
      },
      error: (e) => console.error('Ошибка при получении пользователей: ', e)
    });
  }

  // по разнице выполненных и незавершенных задач
  getUsersSortedByTaskDifference(): void {
    this.userService.getUsersSortedByTaskDifference().subscribe({
      next: (d) => {
        console.log('Получены пользователи:', d);
        this.users = d;
      },
      error: (e) =>console.error('Ошибка при получении пользователей: ', e)
    });
  }
}
