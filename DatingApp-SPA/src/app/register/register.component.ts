import { Component, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
/** register component*/
export class RegisterComponent {
  //@Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  /** register ctor */
  constructor(private authService: AuthService, private alertify: AlertifyService) {

  }

  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log('registration successful');
      this.alertify.success('registration successful');
    }, error => {
      console.log(error);
      this.alertify.error(error);
    });
    console.log(this.model);
  }

  cancel() {
    console.log('cancelled');
    this.cancelRegister.emit(false);
  }
}
