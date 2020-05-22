import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
/** register component*/
export class RegisterComponent implements OnInit {
  //@Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  //model: any = {};
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;

  /** register ctor */
  constructor(private authService: AuthService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router) {

  }
  ngOnInit(): void {
    //this.registerForm = new FormGroup({
    //  username: new FormControl('', Validators.required),
    //  password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //  confirmPassword: new FormControl('', Validators.required)
    //}, this.passwordMatchValidator);

    this.bsConfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'DD/MM/YYYY'
    };
    this.createRegisterForm(); //build the form
  }

  //USING FormBuilder to simplify the way to build a form, basically same way provided by FormGroup in OnInit
  //of course then the code in OnInit can be commented out
  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { 'mismatch': true };
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
      }, error => { //error check
        this.alertify.error(error);
      }, () => { //complete
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });
    }
    //this.authService.register(this.model).subscribe(() => {
    //  console.log('registration successful');
    //  this.alertify.success('registration successful');
    //}, error => {
    //  console.log(error);
    //  this.alertify.error(error);
    //});

    //console.log(this.registerForm.value);
  }

  cancel() {
    console.log('cancelled');
    this.cancelRegister.emit(false);
  }
}
