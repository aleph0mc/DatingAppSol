import { Component } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
/** nav component*/
export class NavComponent {
  model: any = {};

  /** nav ctor */
  constructor(public authService: AuthService, private alertify: AlertifyService) {

  }

  login() {
    console.log(this.model);
    this.authService.login(this.model).subscribe(next => {
      //console.log('Logged in successfully');
      this.alertify.success('Logged in successfully');
    }, error => {
      //console.log(error);
      this.alertify.error(error);
    })
  }

  loggedIn() {
    return this.authService.loggedIn();
    //const token = localStorage.getItem('token');
    //return !!token;
  }

  logout() {
    localStorage.removeItem('token');
    //console.log('Logged out');
    this.alertify.message('Logged out');
  }
}
