package com.hoten.gridia.content;

import java.io.IOException;

interface ContentLoader {

    ContentManager load() throws IOException;
}
