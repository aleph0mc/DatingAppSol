import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
/** member-card component*/
export class MemberCardComponent implements OnInit {
  @Input() userToInport: User;

  /** member-card ctor */
  constructor() { }

  ngOnInit(): void {

  }
}
