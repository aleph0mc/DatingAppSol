import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
/** value component*/
export class ValueComponent {
  values: any;
  /** value ctor */
  constructor(private http: HttpClient) {
    this.getValues();
  }

  getValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      this.values = response;
    }), error => {
      console.log(error);
    }
  }
}
