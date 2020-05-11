import { Component, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

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
  constructor(private authService: AuthService) {

  }

  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log('registration successful');
    }, error => {
      console.log(error);
    });
    console.log(this.model);
  }

  cancel() {
    console.log('cancelled');
    this.cancelRegister.emit(false);
  }
}
