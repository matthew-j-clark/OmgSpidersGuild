import { Component } from '@angular/core';
import { DomSanitizer,SafeResourceUrl } from '@angular/platform-browser'
@Component({
  selector: 'app-streams',
  templateUrl: './streams.component.html'

})
export class StreamsComponent {
  public streamPairList: StreamPair[];

  constructor(private sanitized: DomSanitizer) {
    this.streamPairList = [new StreamPair(sanitized, "SealSlicer","Glovery"),
      new StreamPair(sanitized, "cjcox2056","rahmune" )];
}


}

class StreamPair {
  first : SafeResourceUrl;
  second : SafeResourceUrl;
  constructor(private sanitized: DomSanitizer, firstEntry, secondEntry) {
    this.first = this.createUrl(firstEntry);
    if (secondEntry !== "") {
      this.second = this.createUrl(secondEntry);
    }
  }
  createUrl(entry:string) {
    return this.sanitized.bypassSecurityTrustResourceUrl('https://player.twitch.tv/?channel=' + entry + '&muted=true');
  }
}

