import {
  Directive,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Directive({
  selector: '[appHasRole]', // *appHasRole TO USE IT IN AN HTML TEMPLATE
})
export class HasRoleDirective implements OnInit {
  // we need to pass parameters then in order to use this directive
  // we can use for instance
  // <html-tag *appHasRole="['Admin', 'Moderator']" .....>. . . . <html-tag />
  @Input() appHasRole: string[];
  isVisible = false;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const userRoles = this.authService.decodedToken.roles as Array<string>;
    // if no roles clear the viewContainerRef
    if (!userRoles) {
      this.viewContainerRef.clear();
    }

    // if user has role need then render the element
    if (this.authService.roleMatch(this.appHasRole)) {
      if (!this.isVisible) {
        this.isVisible = true;

        // this statement actually render the html template that contains
        // the structural directive, in this case *appHasRoles
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else { // if user not authorized then do not show the template
        this.isVisible = false;
        this.viewContainerRef.clear();
      }
    }
  }
}
