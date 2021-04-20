import 'bootstrap/dist/css/bootstrap.css';
import React, { Component } from 'react';
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Col from 'react-bootstrap/Col'
import { render } from 'react-dom';
export class Streams extends Component {
    static displayName = Streams.name;

    componentDidMount() {

    }

    static shuffleArray(arr) {
        for (let i = arr.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [arr[i], arr[j]] = [arr[j], arr[i]];
        }   
    }

    static renderStreams() {
        let streamsList = [
            "SealSlicer", "madmonkie44", "mmrrggll420",
            "vrasalstreams", "ztriplex","spookiie__","hexdoesstreaming"
        ];
        this.shuffleArray(streamsList);
        let streamEntries = streamsList.map(this.renderSingleStream)

        return (
            <Container fluid>
                <Row>
                    {streamEntries}
                </Row>
            </Container>
        );
    }

    static renderSingleStream(item, index) {

        let title = item + "Stream";
        let src = "https://player.twitch.tv/?channel=" + item + "&muted=true&parent=omgspiders.com&parent=localhost";        

        return (
            <Col>
                <iframe title={title}
                    src={src}
                    height="300"
                    width="500"
                    scrolling="false"
                    frameBorder="0"
                    allowFullScreen="true" style={{ marginRight: "1em" }} />
            </Col>               
        );

    }



    render() {
        let contents = Streams.renderStreams();

        return (
            <div>
                <h1>OMG Spiders Streams</h1>
                {contents}
            </div>
        );
    }


}