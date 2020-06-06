import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
/** messages component*/
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  /** messages ctor */
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.messages = data['messages'].result; //name in data['...'] must match the name given in routes.ts for the 'resolve' attribute
      this.pagination = data['messages'].pagination;
    });
  }

  loadMessages() {
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PaginatedResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  deleteMesage(id: number, event: any) {
    event.stopPropagation(); // avoid to propagate the click to the tr tag (see html)
    this.alertify.confirm(
      'Are you sure you want to delete this message',
      () => {
        this.userService
          .deleteMessage(this.authService.decodedToken.nameid, id)
          .subscribe(
            () => {
              this.messages.splice(
                this.messages.findIndex((m) => m.id === id),
                1
              );
              this.alertify.success('Message has been deleted');
            },
            (error) => {
              this.alertify.error('Failed to delete the message');
            }
          );
      }
    );
  }
}
