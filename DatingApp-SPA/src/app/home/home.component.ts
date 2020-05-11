import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
/** home component*/
export class HomeComponent {
  registerMode = false;
  //values: any;

  /** home ctor */
  constructor(private http: HttpClient) {
    //this.getValues();
  }

  registerToggle() {
    this.registerMode = true;
  }

  //getValues() {
  //  this.http.get('http://localhost:5000/api/values').subscribe(response => {
  //    this.values = response;
  //  }), error => {
  //    console.log(error);
  //  }
  //}

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
