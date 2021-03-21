import 'bootstrap/dist/css/bootstrap.css';
import React, { Component } from 'react';
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Col from 'react-bootstrap/Col'
export class Streams extends Component {
    static displayName = Streams.name;

    componentDidMount() {

    }

    static renderStreams() {
        let streamsList = [
            "SealSlicer", "madmonkie44", "mmrrggll420",
            "vrasalstreams", "ztriplex"
        ];

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