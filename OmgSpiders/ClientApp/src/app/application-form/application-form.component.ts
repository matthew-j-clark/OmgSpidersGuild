import { Component } from '@angular/core';
import { DomSanitizer,SafeResourceUrl } from '@angular/platform-browser'

@Component({
  selector: 'app-application-form',
  templateUrl: './application-form.component.html',
  styleUrls: ['./application-form.component.css']

})
//
export class ApplicationFormComponent {
  public ApplicationFormUrl : SafeResourceUrl;

  constructor(private sanitized: DomSanitizer) {
    var  appform = "https://docs.google.com/forms/d/1WucSnvGCmSDRbv_N5FVaTnk4RyIHWYY_mRpzTCRUaA8/viewform?embedded=true";
    this.ApplicationFormUrl=this.sanitized.bypassSecurityTrustResourceUrl(appform);

  }

}
