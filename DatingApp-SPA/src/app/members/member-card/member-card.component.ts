import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
/** member-card component*/
export class MemberCardComponent implements OnInit {
  @Input() userToImport: User;

  /** member-card ctor */
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit(): void {

  }

  sendLike() {
    this.userService.sendLike(this.authService.decodedToken.nameid, this.userToImport.id).subscribe(data => {
      this.alertify.success('You have liked: ' + this.userToImport.knownAs);
      console.log('VALUES: ', this.userToImport.id);
    }, error => {
      this.alertify.error(error);
    });
  }

}
