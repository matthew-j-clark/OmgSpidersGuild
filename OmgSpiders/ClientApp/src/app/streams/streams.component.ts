import { Component } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import * as _ from 'lodash';
@Component({
  selector: "app-streams",
  templateUrl: "./streams.component.html"

})
export class StreamsComponent {
  streams:SafeResourceUrl[];
  
  constructor(private readonly sanitized: DomSanitizer) {
    
    let streamsList = [
      "SealSlicer", "Glovery",
      "cjcox2056", "rahmune",
      "Fancyvoidboy", "madmonkie44","mmrrggll420"
    ];

    this.streams =_.shuffle(_.map(streamsList, x=>this.createUrl(x, this.sanitized)));
   
  }
  createUrl(entry: string, sanitizer:DomSanitizer) {
    return sanitizer.bypassSecurityTrustResourceUrl('https://player.twitch.tv/?channel='+entry+'&muted=true');
  }
}
