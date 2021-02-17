import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Welcome to the Home of OMG Spiders - US Sargeras on the Web!</h1>
        <p>Find our latest kill below:</p>
            <iframe title="omg spiders victory list" width="560" height="315" src="https://www.youtube.com/embed/videoseries?list=PLQFewELTbhQC_AKwpOPe3QY3uUgdBDtnk&ab_channel=OMGSpiders" frameBorder="0" allow="autoplay; encrypted-media" allowFullScreen></iframe>
      </div>
    );
  }
}
