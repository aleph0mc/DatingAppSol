import { Component } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
/** nav component*/
export class NavComponent {
  model: any = {};

  /** nav ctor */

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router) { }

  login() {
    console.log(this.model);
    this.authService.login(this.model).subscribe(next => {
      //console.log('Logged in successfully');
      this.alertify.success('Logged in successfully');
    }, error => { //erros
      //console.log(error);
      this.alertify.error(error);
    }, () => { //complete
      this.router.navigate(['/members']);
    });
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
    this.router.navigate(['/home']);
  }
}
