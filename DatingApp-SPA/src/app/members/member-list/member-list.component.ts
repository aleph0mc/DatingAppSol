import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
/** member-list component*/
export class MemberListComponent implements OnInit {
  users: User[];

  /** member-list ctor */
  constructor(private userService: UserService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  //This is called after the constructor when required.
  //The constructor should only be used to initialize class members but shouldn't do actual "work".
  ngOnInit(): void {
    //rather than this
    //this.loadUsers();
    //we can use
    this.route.data.subscribe(data => {
      this.users = data['users']; //name in data['...'] must match the name given in routes.ts for the 'resolve' attribute
    });

  }

  //loadUsers() {
  //  this.userService.getUsers().subscribe((users: User[]) => {
  //    this.users = users;
  //  }, error => {
  //    this.alertify.error(error);
  //  });
  //}
}
