import { Component } from '@angular/core';
import { DomSanitizer,SafeResourceUrl } from '@angular/platform-browser'
@Component({
  selector: 'app-streams',
  templateUrl: './streams.component.html'

})
export class StreamsComponent {
  public streamPairList: StreamPair[];

  constructor(private sanitized: DomSanitizer) {
    // refactor to random order
    this.streamPairList = [];
    var streamsList =["SealSlicer","Glovery",
                 "cjcox2056","rahmune",
                 "Fancyvoidboy","madmonkie44"];
    
    streamsList = shuffle(streamsList);

    var streamsFinal = [];
    for (var i = 0; i < streamsList.length; i+=2) {
      this.streamPairList[i / 2] = new StreamPair(sanitized, streamsList[i], i + 1<streamsList.length? streamsList[i + 1]:undefined);
    }
}
}

function shuffle(array) {
  var currentIndex = array.length, temporaryValue, randomIndex;

  // While there remain elements to shuffle...
  while (0 !== currentIndex) {

    // Pick a remaining element...
    randomIndex = Math.floor(Math.random() * currentIndex);
    currentIndex -= 1;

    // And swap it with the current element.
    temporaryValue = array[currentIndex];
    array[currentIndex] = array[randomIndex];
    array[randomIndex] = temporaryValue;
  }
  
  return array;
}
class StreamPair {
  first : SafeResourceUrl;
  second : SafeResourceUrl;
  constructor(private sanitized: DomSanitizer, firstEntry, secondEntry) {
    this.first = this.createUrl(firstEntry);
    if (secondEntry !== undefined) {
      this.second = this.createUrl(secondEntry);
    }
  }
  createUrl(entry:string) {
    return this.sanitized.bypassSecurityTrustResourceUrl('https://player.twitch.tv/?channel=' + entry + '&muted=true');
  }
}

