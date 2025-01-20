import { Component } from '@angular/core';
import { TodoItemForSearchDto } from '../../models/TodoItemForSearchDto';
import { TodoService } from '../../services/todo/todo.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { User } from '../../models/user.model';
import { TodoType } from '../../models/todoType';
import { UserService } from '../../services/user/user.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-search-todo',
  templateUrl: './search-todo.component.html',
  styleUrl: './search-todo.component.css',
  standalone: true,
  imports: [CommonModule, FormsModule, MatSnackBarModule]
})
export class SearchTodoComponent {
  searchResults: TodoItemForSearchDto[] = [];
  createdAt: Date | null = null;
  typeId: number | null = null;
  authorId: string | null = null;
  users: User[] = []; // список пользователей
  taskTypes: TodoType[] = []; // список задач

  constructor(
    private todoService: TodoService,
    private userService: UserService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadUsers(); // загружаем список пользователей при инциализации компонента
    this.loadTaskTypes(); // загружаем список типов задач
  }

  loadUsers() {
    this.userService.getUsers().subscribe({
      next: (data) => this.users = data,
      error: (err) => {
        console.error('Ошибка получения пользователей:', err)
        this.showError('Не удалось загрусить список пользователей');
      }
    });
  }

  loadTaskTypes() {
    this.todoService.getTaskTypes().subscribe({
      next: (data) => this.taskTypes = data,
      error: (err) => {
        console.error('Ошибка получения типов задач:', err)
        this.showError('Не удалось загрузить список типов задач');
      }
    });
  }


  searchTodos() {

    this.todoService.searchTodos(this.createdAt, this.typeId, this.authorId)
      .subscribe({
        next: (data) => {
          this.searchResults = data;
          if (data.length === 0) {
            this.showInfo('Задачи не найдены.');
          }
        },
        error: (err) => {
          console.error('Ошибка получении задач:', err)
          this.showError('Не удалось загрузить задачи.');
        }
      });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Закрыть', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }

  private showInfo(message: string): void {
    this.snackBar.open(message, 'Закрыть', {
      duration: 3000,
      panelClass: ['info-snackBar']
    });
  }
}
